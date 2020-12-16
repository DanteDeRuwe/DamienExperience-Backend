using DamianTourBackend.Core.Entities;

namespace DamianTourBackend.Application.UpdateWalk
{
    public static class WalkMapper
    {
        public static void UpdateWalk(this WalkDTO model, ref Walk walk) 
        {
            walk.StartTime = model.StartTime != null ? model.StartTime : walk.StartTime;
            walk.EndTime = model.EndTime != null ? walk.EndTime : model.EndTime;
            walk.WalkedPath.Coordinates = model.Coordinates;
            walk.WalkedPath.LineColor = model.LineColor;
        }

        public static Walk MapToWalk(this WalkDTO model) 
        {
            Walk walk = new Walk();
            walk.StartTime = model.StartTime != null ? model.StartTime : walk.StartTime;
            walk.EndTime = model.EndTime != null ? walk.EndTime : model.EndTime;
            walk.SetCoords(model.Coordinates);
            walk.WalkedPath.LineColor = model.LineColor;
            return walk;
        }

        public static WalkDTO MapToWalkDTO(this Walk walk) =>
            new WalkDTO
            {
                ID = walk.Id,
                StartTime = walk.StartTime,
                EndTime = walk.EndTime,
                LineColor = walk.WalkedPath.LineColor,
                Coordinates = walk.WalkedPath.Coordinates
            };
    }
}
