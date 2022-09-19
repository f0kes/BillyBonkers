public struct PlayerInp
{
	public PlayerInp(PlayerInp inp)
	{
		Tick = inp.Tick;
		XMove = inp.XMove;
		ZMove = inp.ZMove;
		Shoot = inp.Shoot;
		ShootReleased = false;
	}
	public int Tick;
	public float XMove;
	public float ZMove;
	public bool Shoot;
	public bool ShootReleased;
}