using PaddleBounceBlockBreak.Components;

namespace PaddleBounceBlockBreak.Systems;

public class ScoreSystem
{
    public void Update(TotalScoreComponent totalScore, HealthComponent health, ScoreComponent score, TextRenderComponent textRender)
    {
        if (health.Health == 0)
        {
            totalScore.TotalScore += score.Score;
        }
        textRender.Text = $"{totalScore.TotalScore}"; // FIXME: This feels encapsulation breaking
    }
}