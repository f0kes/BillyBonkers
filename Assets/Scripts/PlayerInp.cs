using UnityEngine;

public struct PlayerInp
{
	public int Tick;
	public float XMove;
	public float ZMove;
	public bool Shoot;
	public bool ShootReleased;
	public Vector2 MoveStarted;

	public override string ToString()
	{
		return
			$"Tick: {Tick}, XMove: {XMove}, ZMove: {ZMove}, Shoot: {Shoot}, ShootReleased: {ShootReleased}, MoveStarted: {MoveStarted}";
	}
}