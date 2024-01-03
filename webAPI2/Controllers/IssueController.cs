using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

using System.Text.Json.Serialization;
using webAPI2.Data;
using webAPI2.Models;

namespace webAPI2.Controllers
{
    //apply common conventitions 
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly IssueDBContext _issueDBContext;

        public readonly ICachedService _cachedService;


        //public readonly IDistributedCache _cache;
        private string keyname = "Master";
        
        public List<Issue> _issues;

        //public IssueController(IssueDBContext issueDBContext, IDistributedCache distributedCache)
        public IssueController(IssueDBContext issueDBContext, ICachedService CachedService)
        {
            _issueDBContext = issueDBContext;
            _cachedService = CachedService;
        }

        //This is the sync method implemmentation
        // [HttpGet]
        //  public IEnumerable<Issue> SyncGet() => _issueDBContext.Issues.ToList();

        //This is the Async method implemmentation
        [HttpGet]
        // public async Task<IEnumerable<Issue>> AsyncGet() {
        public async Task<IActionResult> AsyncGet()
        {

            var cacheData = _cachedService.GetData < IEnumerable<Issue>>("Issues");

            if (cacheData != null )
            {
                return Ok(cacheData);
            }

            cacheData = await _issueDBContext.Issues.ToListAsync();

            var expiryTime=DateTimeOffset.Now.AddMinutes(5);//we will se about the time again if required

            _cachedService.SetData<IEnumerable<Issue>>("Issues",cacheData,expiryTime);

            return Ok(cacheData);

            //string serialize=string.Empty;

            //var EncodedList=await _cache.GetAsync(keyname);

            /*if (EncodedList != null)
            {

                _issues = new List<Issue>();
                serialize =Encoding.UTF8.GetString(EncodedList);
                _issues = JsonSerializer.Deserialize<List<Issue>>(serialize);

            }
            else { 
            
            _issues= await _issueDBContext.Issues.ToListAsync();

                if (_issues != null)
                {

                    serialize= JsonSerializer.Serialize<List<Issue>>(_issues);
                    EncodedList = Encoding.UTF8.GetBytes(serialize);
                    var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(20)).SetAbsoluteExpiration(TimeSpan.FromHours(6));
                    await _cache.SetAsync(keyname, EncodedList,option);
                }
            }

            return _issues;

            */

        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Issue),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id) { 
            
            var issue= await _issueDBContext.Issues.FindAsync(id);

            return issue == null ? NotFound() : Ok(issue); 
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Issue issue)
        {

            await _issueDBContext.Issues.AddAsync(issue);
            await _issueDBContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById),new { id=issue.Id}, issue);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string id,Issue issue)
        {
            if (id != issue.Id) return BadRequest();

            _issueDBContext.Entry(issue).State = EntityState.Modified;

            await _issueDBContext.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            var issuetoDel =await _issueDBContext.Issues.FindAsync(id);
            if(issuetoDel==null) return NotFound();

            _issueDBContext.Remove(issuetoDel);
            await _issueDBContext.SaveChangesAsync();
            return NoContent();
        }


    }
}
