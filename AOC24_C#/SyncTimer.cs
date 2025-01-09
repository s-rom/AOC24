using Raylib_cs;

namespace Day14;

partial class Day14
{
    struct SyncTimer 
    {
        public double LifeTime;
        public double Start;

        public bool Ended {
            get => Elapsed >= LifeTime;
        }

        public double Elapsed {
            get => Raylib.GetTime() - Start;
        }

        public SyncTimer(double goal)
        {
            LifeTime = goal;
            Start = Raylib.GetTime();
        }

        public void Reset()
        {
            Start = Raylib.GetTime();
        }

    }

}