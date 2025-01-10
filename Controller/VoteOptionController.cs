using FullStackTutorialBackend.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStackTutorialBackend.Controller;

[ApiController]
[Route("[controller]")]
public class VoteOptionController: ControllerBase
{
    private readonly DBContextEF ef;

    public VoteOptionController(IConfiguration config)
    {
        ef = new DBContextEF(config);
    }

    [HttpGet("{voteTopicId}")]
    public async Task<IEnumerable<VoteOption>> GetVoteOption(int voteTopicId)
        => await ef.VoteOptions.Where((option) => option.VoteTopicId == voteTopicId).ToListAsync();

    [HttpPost("{voteTopicId}")]
    public async Task<ActionResult> InsertOptions(int voteTopicId, [FromBody] CreateVoteOptionsDTO optionsDTO)
    {
        var topic = await ef.VoteTopics.FindAsync(voteTopicId);
        if (topic == null) return BadRequest("Cannot Find Topic");

        // No checks for same option already exist
        ef.VoteOptions.AddRange(optionsDTO.Options.Select((option) => new VoteOption {
            VoteTopicId = voteTopicId,
            Content = option
        }));

        await ef.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("Vote/{voteTopicId}/{content}")]
    public async Task<ActionResult> Vote(int voteTopicId, string content)
    {
        var topic = await ef.VoteOptions.FindAsync(voteTopicId, content);
        if (topic == null) return BadRequest("Cannot Find Option");
        topic.TotalVotes++;
        ef.VoteOptions.Update(topic);
        await ef.SaveChangesAsync();

        return Ok();
    }
}