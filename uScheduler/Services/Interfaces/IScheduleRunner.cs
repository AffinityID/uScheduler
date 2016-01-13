using System;

namespace uScheduler.Services.Interfaces
{
    public interface IScheduleRunner
    {
        void Run();
        void Run(int userId, int id);
    }
}