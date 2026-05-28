public class RunningProcess
{
    public SOProcessData data;
    public Entity owner;
    public float timeRemaining;
    public float baseTime;          // needed to recalculate on compute changes
    public EncryptionLevel encryption;
    public bool isServerSide;
}