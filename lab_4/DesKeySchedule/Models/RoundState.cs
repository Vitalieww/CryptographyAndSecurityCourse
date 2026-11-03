namespace DesKeySchedule
{
    public record RoundState(int RoundIndex, int BitsShift, string LeftHalf, string RightHalf, string RoundKey);
}