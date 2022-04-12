﻿using Streamish.Models;
using System.Collections.Generic;

namespace Streamish.Repositories
{
   public interface IVideoRepository
    {
        List<Video> GetAll();
        Video GetById(int id);
        List<Video> GetAllWithComments();
        Video GetVideoByIdWithComments(int id);
        void Add(Video video);
        void Update(Video video);
        void Delete(int id);

    }
}