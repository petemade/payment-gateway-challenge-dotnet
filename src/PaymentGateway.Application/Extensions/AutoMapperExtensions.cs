using System.Linq.Expressions;

using AutoMapper;

namespace PaymentGateway.Application.Extensions;

public static class AutoMapperExtensions
{
    public static IMappingExpression<TSource, TDestination> ForMemberMapFrom<TSource, TDestination, TMember, TSourceMember>(
        this IMappingExpression<TSource, TDestination> expression,
        Expression<Func<TDestination, TMember>> destinationMember,
        Expression<Func<TSource, TSourceMember>> mapExpression)
    {
        return expression.ForMember(destinationMember, opt => opt.MapFrom(mapExpression));
    }
}