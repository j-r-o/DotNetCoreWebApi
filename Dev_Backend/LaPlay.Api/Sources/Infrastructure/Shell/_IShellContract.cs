using System;

namespace LaPlay.Infrastructure.Shell
{
    public interface IShellContract
    {
        string RunCommand(String log);
    }
}