namespace FD.Barking
{
    public interface IBarkReaction
    {
        public bool IsReacting { get; set; }
        public void React(Bark bark);
    }
}
