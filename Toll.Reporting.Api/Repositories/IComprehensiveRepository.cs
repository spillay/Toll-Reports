using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toll.Reporting.Api.DTOs;

public interface IComprehensiveRepository
{
    Task<IEnumerable<ComprehensiveDto>> GetComprehensiveRepositoryAsync(
        DateTime startDate,
        DateTime endDate,
        List<string>? operationalShift = null,
        List<string>? tollOperators = null,
        List<string>? laneNames = null,
        List<string>? laneDiscountTypes = null,
        List<string>? classification = null,
        List<string>? paymentMethods = null,
        List<string>? transactionTypes = null);

  }
