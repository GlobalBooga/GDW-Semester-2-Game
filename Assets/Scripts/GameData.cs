
[System.Serializable]
public class GameData
{
    public GameData()
    {
        weaponID = 1;
        firstTimeInHub = true;
        beatValkyrie = false;
        foundAndarozGun = false;
        beatGluttony = false;
        beatAndaroz = false;
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
