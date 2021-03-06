﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ThreadApp
{
    public class GetAll
    {
        public class Query : IRequest<List<GetAllDTO>>
        {
            public int ForumId { get; set; }
        }

        public class Handler : IRequestHandler<Query, List<GetAllDTO>>
        {
            private readonly AppDbContext _context;

            public Handler(AppDbContext context)
            {
                _context = context;
            }

            public async Task<List<GetAllDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                return await _context.Threads
                    .Where(x => x.ForumFK == request.ForumId)
                    .OrderByDescending(x => x.LastUpdated)
                    .Select(x => new GetAllDTO
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Slug = x.Slug,
                        Author = new GetAllDTO.UserDTO
                        {
                            Id = x.Author.Id,
                            DisplayName = x.Author.DisplayName,
                            AvatarUrl = x.Author.AvatarUrl
                        },
                        CreatedOn = x.CreatedOn,
                        Replies = x.Posts.Count,
                        Views = x.Views,
                        LastReplyDate = x.LastUpdated
                    }).ToListAsync();

                //return await (from threads in _context.Threads
                //              where threads.ForumFK == request.ForumId
                //              orderby threads.LastUpdated descending
                //              select new GetAllDTO
                //              {
                //                  Id = threads.Id,
                //                  Title = threads.Title,
                //                  Slug = threads.Slug,
                //                  Author = (from user in _context.Users
                //                            where user.Id == threads.AuthorFK
                //                            select new GetAllDTO.UserDTO
                //                            {
                //                                Id = user.Id,
                //                                DisplayName = user.DisplayName,
                //                                AvatarUrl = user.AvatarUrl
                //                            }).SingleOrDefault(),
                //                  CreatedOn = threads.CreatedOn,
                //                  Replies = (from post in _context.Posts 
                //                             where post.ThreadFK == threads.Id 
                //                             select post).Count(),
                //                  Views = threads.Views,
                //                  LastReplyDate = threads.LastUpdated
                //              }).ToListAsync();
            }
        }
    }
}
