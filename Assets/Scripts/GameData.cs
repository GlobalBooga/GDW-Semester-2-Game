
[System.Serializable]
public class GameData
{
    public GameData(int gunid, bool foundgun, bool beatambush)
    {
        weaponID = gunid;
        foundAndarozGun = foundgun;
        beatValkyrie = beatambush;
    }

    // player weapon
    public int weaponID = 1;

    // progress
    public bool firstTimeInHub = true;
    public bool beatValkyrie = false;
    public bool foundAndarozGun = false;
    public bool beatGluttony = false;
    public bool beatAndaroz = false;
}
