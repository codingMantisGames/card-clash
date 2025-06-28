using UnityEngine;

namespace CodingMantisGames.SimpleAI
{
    public abstract class ScoreCalculator : ScriptableObject
    {
        public float score;
        public abstract void CalculateScore(AIBrain ai);
    }
}