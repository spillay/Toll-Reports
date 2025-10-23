using Toll.Reporting.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IHourlyTrafficRepository
{
    Task<List<HourlyTrafficDto>> GetHourlyTrafficForSingleDayAsync(
        DateTime startDate,
        DateTime endDate,
        List<string>? classifications = null,
        List<int>? shifts = null,
        bool? operationalDay = null // optional nullable bool
    );
}
