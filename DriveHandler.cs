using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vault
{
    class DriveHandler
    {
        DriveInfo? _driveInfo;

        string _driveLabel;

        public DriveHandler()
        {
            _driveLabel = "VAULT";
        }

        public void scannForDrive()
        {
            _driveInfo = null;

            while (_driveInfo == null)
            {
                findDrive();

                if (_driveInfo == null)
                {
                    Console.WriteLine($"Insert a drive {_driveLabel} and hit the enter");

                    Console.ReadLine();
                }
            }
        }

        public DriveInfo getDriveInfo()
        {
            scannForDrive();
            return _driveInfo;
        }

        private bool findDrive()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in allDrives)
            {
                if (drive.VolumeLabel == _driveLabel)
                {
                    _driveInfo = drive;

                    return true;
                }

            }

            return false;
        }
    }
}
