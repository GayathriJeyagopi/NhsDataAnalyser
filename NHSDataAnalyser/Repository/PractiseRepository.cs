using System.Collections.Generic;
using NHSDataAnalyser.DTO;

namespace NHSDataAnalyser.Repository
{
    /// <summary>
    /// Practise Repository, <seealso cref="AbstractRepository{T}"/>
    /// </summary>
    internal class PractiseRepository : AbstractRepository<Practise>, IPractiseRepository
    {
        public PractiseRepository(IFileParser fileParser) : base(fileParser)
        {
        }

        public override Practise ConstructRepository(IReadOnlyList<string> column)
        {
            var practise = new Practise
            {
                Date = column[0],
                PractiseCode = column[1],
                PractiseName = column[2]
            };

            var address = new Address
            {
                HouseNoBuildingName = column[3],
                Street = column[4],
                City = column[5],
                County = column[6],
                PostCode = column[7]
            };
            practise.Address = address;
            return practise;
        }
    }
}