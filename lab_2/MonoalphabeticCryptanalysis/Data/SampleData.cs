using System.Collections.Generic;

namespace MonoalphabeticCryptanalysis.Data
{
    public static class SampleData
    {
        // Variant 23 raw ciphertext from Laboratory Guide No. 2
        public const string Variant23Ciphertext =
            "Cixvoztg oxo pnzv jvgvwxhp rnil cni qxz, adw, avhtdpv qv rtp qtgofrxwq t htzvit, qv qvsuvo wqv " +
            "hifuwnsnjxpwp rqn rviv snnlxgj cni Athng'phxuqvi pxjgtwdivp xg Pqtlvpuvtiv af ztlxgj uqnwnjituqxh " +
            "vgstijvzvgwpnc wqv Vsxmtavwqtg uixgwxgj wqtw cxjdivo xg wqv rnil. Wqv Ovutiwzvgw " +
            "ncHxuqvip nc wqv Ixkviatgl Stanitwnixvp hngpxpwvo nc 14 ni 15 qxjq-phqnnstgo hnssvjv jitodtwvp " +
            "rqn tppxjgvo wqv xgoxkxodts svwwvip xg wqvpvVsxmtavwqtg wvywp wn ngv ni wqv nwqvi nc wrn " +
            "cngwp nc wfuv tp utiw nc wqvAthngxtg pvtihq. Ctaftg jtkv wqvz wqvxi sxkxgj usdp t ptstif nc " +
            "tandw$50 t zngwq. Wqv pwtcc rtp cvo tgo qndpvo xg Vgjsvovr tgo QnnkviHnwwtjvp, wqv hxuqvi " +
            "stanitwnixvp wtlxgj du wqv cxipw csnni nc Vgjsvovr.Wqv fndgj rnztg rqn hnsstwvo wqv rnil nc ztgf " +
            "nc wqv nwqvi pwtcczvzavip rtp Vsxmvavwq Pzxwq. Pqv qto avvg , anig Tdjdpw 26, 1892, " +
            "xgQdgwxgjwng, Xgoxtgt, wqv fndgjvpw nc wqv gxgv hqxsoivg nc Enqg Z. Pzxwq,t otxifztg, atglvi, " +
            "tgo hndgwf Ivudasxhtg hnzzxwwvvztg, tgo qxprxcv, Pnuqt, rqn puvssvo qvi otdjqwvi'p Hqixpwxtg " +
            "gtzv rxwa tg vxgpwvto nc tg t xg wqv zxoosv avhtdpv pqv rtp gnw jnxgj wn qtkv tgfngvhtssxgj qvi " +
            "hqxso \"Vsxmt.\" Tcwvi hnzusvwxgj qxjq phqnns xg Qdgwxgjwng,Vsxmvavwq twwvgovo Rnnpwvi " +
            "Hnssvjv aixvcsf adw rtp jitodtwvo cinzQxsspotsv Hnssvjv xg Zxhqxjtg rqviv pqv qto ztenivo xg " +
            "Vgjsxpq. Rqxsvrnilxgj tw wqv Gvraviif Sxaitif xg Hqxhtjn, pqv rtp ivhidxwvo afCtaftg tgo avjtg rnil " +
            "wqviv xg 1916.Gvxwqvi pqv gni Cixvoztg qto jxkvg tgf utiwxhdsti uivkxndp wqndjqwwn hifuwnsnjf, " +
            "adw wqvf avjtg wn jvw uvipngtssf xgwvivpwvo xg wqv rnil. Xwxp fvw tgnwqvi nc wqv xingxvp nc " +
            "hifuwnsnjxh qxpwnif wqtw wqv xgwvivpw nc wrncnivznpw hifuwnsnjxpwp rtp tindpvo af t ctspv " +
            "onhwixgv—t onhwixgv,znivnkvi, tjtxgpw rqxhq wqvf stwvi rviv wn rtjv t sxcvwxzv atwwsv. Cni " +
            "twwtasv tw wqv Ixkviatgl hnwwtjvp wqvf qvtio jtdof wtsvp nc sdpwfVsxmtavwqtg sxcv, nc wqv " +
            "gnw-pn-Kxijxg Bdvvg, nc hndiwxvip' xgwixjdvp tgowqv pvhivw qxpwnixvp nc wqv jivtw gtzvp nc " +
            "Vgjsxpq qxpwnif—tss thwdtssfxgktsxo ovhxuqvizvgwp nc Pqtlvpuvtiv'p ustfp wvgoxgj wn uinkv " +
            "wqtwAthng qto rixwwvg wqvz, ivstwvo af wqv jvgwsv, duixjqw, adw pvsc-ovsdovornztg rqn qto " +
            "\"ovhxuqvivo\" wqvz, Zip. Vsxmtavwq Rvssp Jtssdu. Wqvpvpwnixvp pwxiivo Cixvoztg'p oniztgw " +
            "xgwvivpw; qv avjtg wn on pnzv nc wqvhifuwnsnjf, tgo xgvkxwtasf xwp udxpptgw ztjxh pvvuvo sxlv " +
            "wqv cdzv ncunuuxvp xgwn qxp zxgo tgo puxixw tgo xgwnyxhtwvo qxz.\"Rqvg xw htzv wn wqv " +
            "hifuwnsnjf,\" qv ivhtssvo fvtip stwvi,\"pnzvwqxgj xg zv cndgo tg ndwsvw.\"Tg dgovipwtwvzvgw. Qv " +
            "pnng cndgo qxzpvsc qvto nc wqv Ovutiwzvgw ncHxuqvip tp rvss tp wqv Ovutiwzvgw nc Jvgvwxhp " +
            "tw Ixkviatgl. Wqvtwwithwxng qv cvsw cni hifuwnsnjf rtp ivxgcnihvo af wqv twwithwxng qv cvsw " +
            "cnit hifuwnsnjxpw: wqv bdxhl-rxwwvo tgo puixjqwsf Zxpp Pzxwq. Xg Ztf nc 1917wqvf rviv ztiixvo " +
            "tgo pwtiwvo wqv znpw ctzndp qdpatgo-tgo-rxcv wvtzxg wqv qxpwnif nc hifuwnsnjf.Tzvixht qto " +
            "ovhstivo rti t zngwq avcniv, tgo Ixkvi-atgl, rqxhq qtowqv ngsf jnxgj hifuwnsnjxh hnghvig xg wqv " +
            "hndgwif, avjtg jvwwxgj, ng tgxgcnizts atpxp, hifuwnjitzp cni pnsdwxng cinz ktixndp " +
            "jnkvigzvgwadivtdp. Uinatasf wqv znpw xzuniwtgw rviv zvpptjvp wn tgo cinz t ixgjnc 125 Qxgodp " +
            "rqn, rxwq Jviztg txo, rviv wtlxgj toktgwtjv ncVgjstgo'p uivnhhdutwxng xg Vdinuv wn pwixlv cni " +
            "Xgoxtg xgovuvgovghv.Wqv xgwvihvuwp rviv jxkvg wn Cixvoztg cni pnsdwxng, tgo qv bdxhlsf " +
            "pnskvowqv gdzavi hxuqvi dpvo xg htasvjitzp wn Avisxg. Wqv svwwvip nc wqvustxgwvyw tgo nc " +
            "wqv lvfrnio rviv witgpcnizvo xgwn oxjxwp af zvtgp nc t4 Y 7 hqvhlviantio rxwq t gnizts tsuqtavw; " +
            "wqv lvf oxjxwp rviv wqvgtoovo wn wqnpv nc wqv ustxgwvyw wn cniz wqv hxuqviwvyw. Ngv lvf " +
            "rtpSTZU. Vthq tjvgw qto qxp nrg lvf, adw Cixvoztg qto gn windasv xgpnskxgj wqvz. Gni rtp qv " +
            "pwdzuvo af t pfpwvz dpdtssf ivjtiovo aftztwvdip tp wqv gv usdp dswit nc hifuwnjituqxh pvhdixwf: t " +
            "annl hxuqvi.";

        // Standard English letter frequencies from Laboratory Guide Table 2.2 / Figure 2.2
        public static readonly IReadOnlyDictionary<char, double> StandardEnglishFrequencies =
            new Dictionary<char, double>
            {
                { 'A', 8.17 }, { 'B', 1.49 }, { 'C', 2.78 }, { 'D', 4.25 },
                { 'E', 12.70 }, { 'F', 2.23 }, { 'G', 2.01 }, { 'H', 6.09 },
                { 'I', 6.97 }, { 'J', 0.15 }, { 'K', 0.77 }, { 'L', 4.03 },
                { 'M', 2.41 }, { 'N', 6.75 }, { 'O', 7.51 }, { 'P', 1.93 },
                { 'Q', 0.09 }, { 'R', 5.99 }, { 'S', 6.33 }, { 'T', 9.06 },
                { 'U', 2.76 }, { 'V', 0.98 }, { 'W', 2.36 }, { 'X', 0.15 },
                { 'Y', 1.97 }, { 'Z', 0.07 }
            };

        // Common English Digraphs (ordered roughly by frequency)
        public static readonly string[] CommonDigraphs =
            ["TH", "HE", "IN", "ER", "AN", "RE", "ED", "ON", "ES", "ST", "EN", "AT", "TO", "NT", "HA", "ND", "OU", "EA", "NG", "AS", "OR", "TI", "IS", "ET", "IT", "AR", "TE", "SE", "HI", "OF"];

        // Common English Trigraphs
        public static readonly string[] CommonTrigraphs =
            ["THE", "AND", "THA", "ENT", "ION", "TIO", "FOR", "NDE", "HAS", "NCE", "TIS", "OFT", "MEN"];

        // Common English Double Letters
        public static readonly string[] CommonDoubleLetters =
            ["SS", "EE", "TT", "OO", "FF", "LL", "MM", "PP"];
    }
}
