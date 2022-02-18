using System;

[System.Serializable]
public class VersionData
{
    public int major, minor, patch;

    public void CheckFormatting()
    {
        if (major > 9999)
        {
            major = 9999;
        }
        if (minor > 99)
        {
            minor = 99;
        }
        if (patch > 99)
        {
            patch = 99;
        }
    }
    public string ToString(char separator = '-')
    {
        return $"{major.ToString("0000")}{separator}{minor.ToString("00")}{separator}{patch.ToString("00")}";
    }

    public void SetStartDate()
    {
        if (major + minor + patch == 0)
        {
            major = DateTime.Now.Year;
            minor = DateTime.Now.Month;
            patch = DateTime.Now.Day;
        }
    }
}