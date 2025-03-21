public enum Job 
{ 
    Unassigned, 
    Farmer, 
    Miner, 
    Lumberjack, 
    Builder, 
    Guard, 
    Adventurer, 
    Merchant, 
    Craftsman 
}

public enum LifeStage 
{ 
    Child, 
    Adult, 
    Elder 
}

public enum ResourceTypes
{
    Wood,
    Stone,
    Food,
    Gold
}

public enum Rank 
{ S, A, B, C, D, E, F }

public enum UnitState 
{ 
    Idle, 
    WalkingToWork, 
    Working, 
    WalkingHome 
}

public enum EnemyState
{
    Idle,
    Moving,
    Attacking,
    Dead
}