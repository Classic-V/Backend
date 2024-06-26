﻿using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Services.Animation.Interface;

public interface IAnimationService
{
    List<AnimationModel> Animations { get; }

    Task<AnimationModel> GetAnimation(int id);
    Task AddAnimation(AnimationModel model);
}