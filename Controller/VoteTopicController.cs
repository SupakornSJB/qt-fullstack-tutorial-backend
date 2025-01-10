using FullStackTutorialBackend.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FullStackTutorialBackend.Controller;

[ApiController]
[Route("[controller]")]
public class VoteTopicController: ControllerBase
{
    private readonly DBContextEF ef;

    public VoteTopicController(IConfiguration config)
    {
        ef = new DBContextEF(config);
    }

    [HttpGet]
    public async Task<IEnumerable<VoteTopic>> GetVoteTopics(int? lastTopicId, int amount = 10)
    {
        IQueryable<VoteTopic> topics = ef.VoteTopics.AsQueryable<VoteTopic>();
        if (lastTopicId != null)
            topics = topics.Where((t) => t.Id > lastTopicId);
        topics = topics.Take(amount);
        return await topics.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<VoteTopic>> InsertVoteTopic([FromBody] CreateVoteTopicDTO topicDTO)
    {
        var topic = await ef.VoteTopics.AddAsync(new VoteTopic {
            Title = topicDTO.Title,
            Description = topicDTO.Description
        });
        await ef.SaveChangesAsync();
        return Ok(topic.Entity);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateVoteTopic([FromBody] VoteTopic topic) 
    {
        var result = await ef.VoteTopics.FindAsync(topic.Id);
        if (result == null) return NotFound();
        ef.VoteTopics.Update(topic);
        await ef.SaveChangesAsync();
        return Ok();
    }
}