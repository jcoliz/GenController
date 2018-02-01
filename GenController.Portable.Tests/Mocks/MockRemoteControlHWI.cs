using GenController.Portable.Models;
using System;

namespace GenController.Portable.Tests.Mocks
{
    public class MockRemoteControlHWI : IRemote
    {
        public bool IsPressed(int line)
        {
            if (line < 1 || line > 4)
                throw new ArgumentException("Argument out of range", nameof(line));

            return LineStatus[line-1];
        }
        public void SetPressed(int line,bool value)
        {
            if (line < 1 || line > 4)
                throw new ArgumentException("Argument out of range", nameof(line));

            if (value != LineStatus[line-1])
            {
                LineStatus[line - 1] = value;
                LineChanged.Invoke(this, line);
            }
        }

        public event EventHandler<int> LineChanged;

        private bool[] LineStatus = new bool[] { false, false, false, false };
    }
}
