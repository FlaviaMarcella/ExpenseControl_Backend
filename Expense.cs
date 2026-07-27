namespace ExpenseControl.Api;

public class Transaction
{
    private int _id {get; set;}
    private string _description {get; set;}
    private decimal _amount {get; set;}
    private DateOnly _date {get; set;}
    private TypeTransaction type {get; set;}
    private People _people {get; set;}
    
}