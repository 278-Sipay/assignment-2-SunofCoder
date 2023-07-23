using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SipayApi.Base;
using SipayApi.DataAccess.Domain;
using SipayApi.DataAccess.Repository;
using SipayApi.Schema;
using System.Linq.Expressions;

namespace SipayApi;

[ApiController]
[Route("sipy/api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionRepository repository;
    private readonly IMapper mapper;
    public TransactionController(ITransactionRepository repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    [HttpGet]
    public ApiResponse<List<TransactionResponse>> GetAll()
    {
        var entityList = repository.GetAll();
        var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
        return new ApiResponse<List<TransactionResponse>>(mapped);
    }

    [HttpGet("{id}")]
    public ApiResponse<TransactionResponse> Get(int id)
    {
        var entity = repository.GetById(id);
        var mapped = mapper.Map<Transaction, TransactionResponse>(entity);
        return new ApiResponse<TransactionResponse>(mapped);
    }

    [HttpGet("GetByReference")]
    public ApiResponse<List<TransactionResponse>> GetByReference(string ReferenceNumber)
    {
        var entityList = repository.GetByReference(ReferenceNumber);
        var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
       return new ApiResponse<List<TransactionResponse>>(mapped);
    }

    [HttpPost]
    public ApiResponse Post([FromBody] TransactionRequest request)
    {
        var entity = mapper.Map<TransactionRequest, Transaction>(request);
        repository.Insert(entity);
        repository.Save();
        return new ApiResponse();
    }

    [HttpGet("GetByParameter")]
    public ApiResponse<List<TransactionResponse>> GetByParameter(
     int? accountNumber = null,
     string referenceNumber = null,
     decimal? minCreditAmount = null,
     decimal? maxCreditAmount = null,
     decimal? minDebitAmount = null,
     decimal? maxDebitAmount = null,
     string description = null,
     DateTime? beginDate = null,
     DateTime? endDate = null)
    {
        Expression<Func<Transaction, bool>> filterExpression = x =>
            (!accountNumber.HasValue || x.AccountNumber == accountNumber.Value) &&
            (string.IsNullOrEmpty(referenceNumber) || x.ReferenceNumber == referenceNumber) &&
            (!minCreditAmount.HasValue || x.CreditAmount >= minCreditAmount) &&
            (!maxCreditAmount.HasValue || x.CreditAmount <= maxCreditAmount) &&
            (!minDebitAmount.HasValue || x.DebitAmount >= minDebitAmount) &&
            (!maxDebitAmount.HasValue || x.DebitAmount <= maxDebitAmount) &&
            (string.IsNullOrEmpty(description) || x.Description.Contains(description)) &&
            (!beginDate.HasValue || x.TransactionDate >= beginDate) &&
            (!endDate.HasValue || x.TransactionDate <= endDate);

        
        var entityList = repository.GetByParameter(filterExpression);
        var mapped = mapper.Map<List<Transaction>, List<TransactionResponse>>(entityList);
        return new ApiResponse<List<TransactionResponse>>(mapped);
    }
}