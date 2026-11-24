#!/usr/bin/env bash
# ==============================================================================
# Internal PKI Management Script using OpenSSL
# Laboratory Work No. 5 - Public Key Infrastructure & Digital Signatures (DSA)
# ==============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BASE_DIR="${SCRIPT_DIR}/../pki_store"

CA_DIR="${BASE_DIR}/ca"
CERTS_DIR="${CA_DIR}/certs"
CRL_DIR="${CA_DIR}/crl"
NEW_CERTS_DIR="${CA_DIR}/newcerts"
PRIVATE_DIR="${CA_DIR}/private"
USERS_DIR="${BASE_DIR}/users"
CNF_FILE="${CA_DIR}/openssl.cnf"
TEMPLATE_FILE="${SCRIPT_DIR}/openssl.cnf.template"

header() {
    echo -e "\n\033[1;36m========================================================\033[0m"
    echo -e "  \033[1;33m$1\033[0m"
    echo -e "\033[1;36m========================================================\033[0m"
}

check_openssl() {
    if ! command -v openssl >/dev/null 2>&1; then
        echo -e "\033[1;31mError: OpenSSL is not installed or not in PATH.\033[0m"
        exit 1
    fi
}

init_pki_structure() {
    mkdir -p "${CERTS_DIR}" "${CRL_DIR}" "${NEW_CERTS_DIR}" "${PRIVATE_DIR}" "${USERS_DIR}"
    [ -f "${CA_DIR}/index.txt" ] || touch "${CA_DIR}/index.txt"
    [ -f "${CA_DIR}/serial" ] || echo "1000" > "${CA_DIR}/serial"
    [ -f "${CA_DIR}/crlnumber" ] || echo "1000" > "${CA_DIR}/crlnumber"

    sed "s|__CA_DIR__|${CA_DIR}|g" "${TEMPLATE_FILE}" > "${CNF_FILE}"
}

initialize_ca() {
    header "Initialize Root Certified Authority (CA)"
    init_pki_structure

    local ca_key="${PRIVATE_DIR}/ca.key"
    local ca_crt="${CERTS_DIR}/ca.crt"
    local ca_crl="${CRL_DIR}/ca.crl"

    if [ -f "${ca_crt}" ]; then
        echo -e "\033[1;33mCA is already initialized!\033[0m"
        read -rp "Do you want to re-initialize and wipe existing CA data? (y/N): " recreate
        if [[ ! "${recreate}" =~ ^[Yy]$ ]]; then
            echo "Aborted CA initialization."
            return
        fi
        rm -rf "${CA_DIR:?}"/*
        init_pki_structure
    fi

    echo -e "\033[1;32mGenerating CA Root Private Key (RSA 4096-bit)...\033[0m"
    openssl genpkey -algorithm RSA -pkeyopt rsa_keygen_bits:4096 -out "${ca_key}"

    echo -e "\033[1;32mGenerating Self-Signed CA Certificate (Validity: 3650 days / 10 years)...\033[0m"
    local subj="/C=MD/ST=Chisinau/L=Chisinau/O=Technical University of Moldova/OU=FAF/CN=UTM-FAF-Root-CA"
    openssl req -config "${CNF_FILE}" -key "${ca_key}" -new -x509 -days 3650 -sha256 -extensions v3_ca -out "${ca_crt}" -subj "${subj}"

    echo -e "\033[1;32mGenerating Initial Certificate Revocation List (CRL)...\033[0m"
    openssl ca -config "${CNF_FILE}" -gencrl -out "${ca_crl}"

    echo -e "\n\033[1;32mRoot CA successfully initialized!\033[0m"
    echo "CA Key:         ${ca_key}"
    echo "CA Certificate: ${ca_crt}"
    echo "CA CRL:         ${ca_crl}"
}

issue_user_cert() {
    header "Issue User Private Key & Certificate"
    local ca_crt="${CERTS_DIR}/ca.crt"
    if [ ! -f "${ca_crt}" ]; then
        echo -e "\033[1;31mRoot CA is not initialized! Please run option 1 first.\033[0m"
        return
    fi

    read -rp "Enter username / entity name (e.g. alice, bob): " username
    username="$(echo "${username}" | xargs)"
    if [ -z "${username}" ]; then
        echo -e "\033[1;31mInvalid username.\033[0m"
        return
    fi

    read -rp "Enter RSA key dimension (2048, 3072, 4096) [Default: 2048]: " key_bits
    key_bits="${key_bits:-2048}"
    if [ "${key_bits}" != "2048" ] && [ "${key_bits}" != "3072" ] && [ "${key_bits}" != "4096" ]; then
        echo -e "\033[1;33mKey size must be minimum 2048 bits. Setting to 2048.\033[0m"
        key_bits=2048
    fi

    local user_path="${USERS_DIR}/${username}"
    mkdir -p "${user_path}"

    local user_key="${user_path}/${username}.key"
    local user_csr="${user_path}/${username}.csr"
    local user_crt="${user_path}/${username}.crt"
    local user_pub="${user_path}/${username}.pub"

    echo -e "\n\033[1;32m1. Generating User RSA Private Key (${key_bits}-bit)...\033[0m"
    openssl genpkey -algorithm RSA -pkeyopt "rsa_keygen_bits:${key_bits}" -out "${user_key}"

    echo -e "\033[1;32m2. Creating Certificate Signing Request (CSR)...\033[0m"
    local subj="/C=MD/ST=Chisinau/O=Technical University of Moldova/OU=FAF/CN=${username}"
    openssl req -config "${CNF_FILE}" -new -key "${user_key}" -out "${user_csr}" -subj "${subj}"

    echo -e "\033[1;32m3. CA Signing User Certificate (Validity: 365 days)...\033[0m"
    openssl ca -config "${CNF_FILE}" -batch -extensions usr_cert -days 365 -notext -md sha256 -in "${user_csr}" -out "${user_crt}"

    echo -e "\033[1;32m4. Extracting Public Key for signature verification...\033[0m"
    openssl x509 -in "${user_crt}" -pubkey -noout -out "${user_pub}"

    echo -e "\n\033[1;32mUser Certificate & Key successfully issued!\033[0m"
    echo "Private Key: ${user_key}"
    echo "Certificate: ${user_crt}"
    echo "Public Key:  ${user_pub}"
}

revoke_user_cert() {
    header "Revoke User Certificate"
    read -rp "Enter username to revoke: " username
    local user_crt="${USERS_DIR}/${username}/${username}.crt"

    if [ ! -f "${user_crt}" ]; then
        echo -e "\033[1;31mCertificate not found at ${user_crt}\033[0m"
        return
    fi

    read -rp "Enter revocation reason [Default: keyCompromise]: " reason
    reason="${reason:-keyCompromise}"

    echo -e "\n\033[1;32m1. Revoking certificate in CA database...\033[0m"
    openssl ca -config "${CNF_FILE}" -revoke "${user_crt}" -crl_reason "${reason}"

    echo -e "\033[1;32m2. Regenerating Certificate Revocation List (CRL)...\033[0m"
    local ca_crl="${CRL_DIR}/ca.crl"
    openssl ca -config "${CNF_FILE}" -gencrl -out "${ca_crl}"

    echo -e "\n\033[1;32mCertificate for '${username}' has been REVOKED and CRL updated!\033[0m"
}

sign_document() {
    header "Sign Document / File (Digital Signature)"
    read -rp "Enter signer username: " username
    local user_key="${USERS_DIR}/${username}/${username}.key"
    local user_crt="${USERS_DIR}/${username}/${username}.crt"

    if [ ! -f "${user_key}" ] || [ ! -f "${user_crt}" ]; then
        echo -e "\033[1;31mUser key or certificate not found for '${username}'.\033[0m"
        return
    fi

    read -rp "Enter path to file/document to sign: " file_path
    if [ ! -f "${file_path}" ]; then
        echo -e "\033[1;31mFile not found: ${file_path}\033[0m"
        return
    fi

    local sig_path="${file_path}.sig"

    echo -e "\033[1;32mSigning file '${file_path}' using SHA-256 and RSA private key of '${username}'...\033[0m"
    openssl dgst -sha256 -sign "${user_key}" -out "${sig_path}" "${file_path}"

    echo -e "\n\033[1;32mDocument successfully signed!\033[0m"
    echo "Original Document: ${file_path}"
    echo "Digital Signature: ${sig_path}"
    echo "Signer Certificate: ${user_crt}"
}

verify_signature() {
    header "Verify Digital Signature & Certificate Validity"
    read -rp "Enter path to original document: " file_path
    if [ ! -f "${file_path}" ]; then
        echo -e "\033[1;31mFile not found: ${file_path}\033[0m"
        return
    fi

    read -rp "Enter path to signature file [Default: ${file_path}.sig]: " sig_path
    sig_path="${sig_path:-${file_path}.sig}"
    if [ ! -f "${sig_path}" ]; then
        echo -e "\033[1;31mSignature file not found: ${sig_path}\033[0m"
        return
    fi

    read -rp "Enter signer username (or path to signer certificate): " signer_input
    local cert_path=""
    if [ -f "${signer_input}" ]; then
        cert_path="${signer_input}"
    else
        cert_path="${USERS_DIR}/${signer_input}/${signer_input}.crt"
    fi

    if [ ! -f "${cert_path}" ]; then
        echo -e "\033[1;31mSigner certificate not found: ${cert_path}\033[0m"
        return
    fi

    echo -e "\n\033[1;36m--- Step 1: Cryptographic Signature Verification ---\033[0m"
    local temp_pub
    temp_pub="$(mktemp)"
    openssl x509 -in "${cert_path}" -pubkey -noout > "${temp_pub}"

    local sig_ok=0
    if openssl dgst -sha256 -verify "${temp_pub}" -signature "${sig_path}" "${file_path}" >/dev/null 2>&1; then
        sig_ok=1
        echo -e "\033[1;32m[+] Cryptographic Signature is VALID!\033[0m"
        echo "    The document has NOT been tampered with and was signed by the key holder."
    else
        echo -e "\033[1;31m[-] Cryptographic Signature is INVALID!\033[0m"
        echo "    The document or signature was modified or does not match this key."
    fi
    rm -f "${temp_pub}"

    echo -e "\n\033[1;36m--- Step 2: PKI Certificate Trust & Revocation Check ---\033[0m"
    local ca_crt="${CERTS_DIR}/ca.crt"
    local ca_crl="${CRL_DIR}/ca.crl"

    local cert_ok=0
    set +e
    local verify_output
    verify_output="$(openssl verify -CAfile "${ca_crt}" -CRLfile "${ca_crl}" -crl_check "${cert_path}" 2>&1)"
    local verify_code=$?
    set -e

    if [ ${verify_code} -eq 0 ] && echo "${verify_output}" | grep -q ": OK"; then
        cert_ok=1
        echo -e "\033[1;32m[+] Signer Certificate is TRUSTED and VALID!\033[0m"
        echo "    - Issued by our Root CA"
        echo "    - Within validity period (365 days)"
        echo "    - NOT revoked in CRL"
    else
        echo -e "\033[1;31m[-] Signer Certificate Verification FAILED!\033[0m"
        if echo "${verify_output}" | grep -q "certificate revoked"; then
            echo -e "    \033[1;31m[!] REASON: The certificate has been REVOKED by the CA!\033[0m"
        else
            echo "    Details: ${verify_output}"
        fi
    fi

    echo -e "\n\033[1;36m--- Overall Status ---\033[0m"
    if [ ${sig_ok} -eq 1 ] && [ ${cert_ok} -eq 1 ]; then
        echo -e "\033[1;32m>>> OVERALL RESULT: FULLY TRUSTED & VALID DIGITAL SIGNATURE <<<\033[0m"
    elif [ ${sig_ok} -eq 1 ] && [ ${cert_ok} -eq 0 ]; then
        echo -e "\033[1;33m>>> OVERALL RESULT: UNTRUSTED / REVOKED SIGNATURE (Key matches, but certificate is INVALID) <<<\033[0m"
    else
        echo -e "\033[1;31m>>> OVERALL RESULT: VERIFICATION REJECTED <<<\033[0m"
    fi
}

list_certificates() {
    header "List PKI Certificates & CA Database"
    local index_file="${CA_DIR}/index.txt"
    if [ ! -f "${index_file}" ] || [ ! -s "${index_file}" ]; then
        echo -e "\033[1;33mNo certificates have been issued yet.\033[0m"
        return
    fi

    printf "%-8s | %-15s | %-18s | %-6s | %s\n" "STATUS" "EXPIRATION" "REVOCATION DATE" "SERIAL" "SUBJECT"
    echo "--------------------------------------------------------------------------------"
    while IFS=$'\t' read -r status exp rev serial file subj || [ -n "${status}" ]; do
        if [ "${status}" = "V" ]; then
            printf "\033[1;32m%-8s\033[0m | %-15s | %-18s | %-6s | %s\n" "VALID" "${exp}" "N/A" "${serial}" "${file}"
        elif [ "${status}" = "R" ]; then
            printf "\033[1;31m%-8s\033[0m | %-15s | %-18s | %-6s | %s\n" "REVOKED" "${exp}" "${rev}" "${serial}" "${subj}"
        else
            printf "%-8s | %-15s | %-18s | %-6s | %s\n" "${status}" "${exp}" "${rev}" "${serial}" "${subj}"
        fi
    done < "${index_file}"
}

check_openssl

while true; do
    header "Public Key Infrastructure (PKI) & Digital Signatures (DSA)"
    echo "1. Initialize Root Certified Authority (CA) [4096-bit RSA, 10 Years]"
    echo "2. Issue User Private Key & Certificate [2048+ bit RSA, 365 Days]"
    echo "3. Revoke User Certificate & Update CRL"
    echo "4. Sign Document / File"
    echo "5. Verify Document Signature & PKI Trust Chain"
    echo "6. List All Issued Certificates"
    echo "7. Exit"
    echo "--------------------------------------------------------"
    read -rp "Select an option (1-7): " choice

    case "${choice}" in
        1) initialize_ca ;;
        2) issue_user_cert ;;
        3) revoke_user_cert ;;
        4) sign_document ;;
        5) verify_signature ;;
        6) list_certificates ;;
        7) echo "Goodbye!"; exit 0 ;;
        *) echo -e "\033[1;31mInvalid option. Choose 1-7.\033[0m" ;;
    esac
done
