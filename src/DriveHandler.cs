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
                    ConsoleManager.ShowMessage($"Insert a drive {_driveLabel} and hit the enter");

                    ConsoleManager.GetInput();
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
