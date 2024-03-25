namespace DAL.Annotation
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public abstract class BaseEntityAnnotation<T> : IEntityAnnotation
        where T : class
    {
        protected BaseEntityAnnotation(ModelBuilder builder)
        {
            this.ModelBuilder = builder.Entity<T>();
        }

        protected EntityTypeBuilder<T> ModelBuilder { get; }

        public abstract void Annotate();
    }
}
