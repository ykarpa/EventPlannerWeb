// <copyright file="EventPlannerContextFactory.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace EventPlannerWeb.Data
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class EventPlannerContextFactory : IDesignTimeDbContextFactory<EventPlannerContext>
    {
        /// <inheritdoc/>
        public EventPlannerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EventPlannerContext>();

            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Username=postgres;Password=vitalik;Database=EventPlannerWeb");

            return new EventPlannerContext(optionsBuilder.Options);
        }
    }
}
