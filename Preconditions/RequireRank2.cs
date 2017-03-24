﻿using DEA.SQLite.Models;
using DEA.SQLite.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireRank2Attribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IDependencyMap map)
        {
            if (Config.SPONSOR_IDS.Any(x => x == context.User.Id)) return PreconditionResult.FromSuccess();
            using (var db = new DbContext())
            {
                var guildRepo = new GuildRepository(db);
                var userRepo = new UserRepository(db);
                var user = await context.Guild.GetUserAsync(context.User.Id) as IGuildUser;
                var role2Id = await guildRepo.GetRank2Id(context.Guild.Id);
                if (context.Guild.GetRole(role2Id) == null)
                    return PreconditionResult.FromError($"This command may not be used if the second rank role does not exist.\n" +
                                                        $"Use the `{await guildRepo.GetPrefix(context.Guild.Id)}SetRankRoles` command to change that.");
                if (user.RoleIds.All(x => x != role2Id))
                    return PreconditionResult.FromError($"You do not have the permission to use this command.\nRequired role: {context.Guild.GetRole(role2Id).Mention}");
                if (await userRepo.GetCash(user.Id) < Config.RANK2)
                    return PreconditionResult.FromError("Hmmm.... It seems you did not get that rank legitimately.");
                return PreconditionResult.FromSuccess();
            }
        }
    }
}