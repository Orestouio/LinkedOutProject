using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BackendApp.Model;
using BackendApp.Model.Enums;
using BackendApp.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BackendApp.Auth.AuthConstants.PolicyNames;


namespace BackendApp.Controller
{

    [Route("api/[Controller]")]
    [ApiController]
    public class LinkedOutPostController
    (ILinkedOutPostService postService, IInterestService interestService) 
    : ControllerBase
    {
        private readonly ILinkedOutPostService postService = postService;
        private readonly IInterestService interestService = interestService;

        [HttpPost]
        public IActionResult CreatePost(Post post)
            => this.postService.AddPost(post) ? this.Ok(post.Id) : this.Conflict();
        
        [Route("create/{userId}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName)]
        public IActionResult CreatePost(ulong userId, string content)
            => this.postService.AddPost(content, userId) ? this.Ok() : this.Conflict();
        
        [Route("{id}")]
        [HttpPost]
        public IActionResult UpdatePost(ulong id, Post post)
            => this.postService.UpdatePost(id, post) switch
            {
                UpdateResult.KeyAlreadyExists => this.Conflict(),
                UpdateResult.NotFound => this.NotFound(),
                UpdateResult.Ok => this.Ok(),
                _  => throw new Exception("Something went terribly wrong for you to be here.") 
            };
        
        [Route("{id}")]
        [HttpDelete]
        [Authorize]
        public IActionResult Delete(ulong id)
            => this.postService.RemovePost(id) ? this.Ok() : this.NotFound();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
            => this.Ok(this.postService.GetAllPosts());

        [Route("{id}")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get(ulong id)
        {
            var user = this.postService.GetPostById(id);
            return user is not null ? this.Ok(user) : this.NotFound();
        }

        [Route("{postId}/interest/set/{userId}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        public IActionResult DeclareInterest(uint userId, uint postId)
        {
            return this.interestService.DeclareInterestForPost(userId, postId).ToResultObject(this);
        }

        [Route("{postId}/interest/unset/{userId}")]
        [HttpPost]
        [Authorize( Policy = HasIdEqualToUserIdParamPolicyName )]
        public IActionResult RemoveInterest(uint userId, uint postId)
        {
            return this.interestService.RemoveInterestForPost(userId, postId).ToResultObject(this);
        }
    }
}