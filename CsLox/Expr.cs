namespace CsLox;

public abstract class Expr {
  public interface IVisitor<TResult> {
    public TResult VisitBinaryExpr(Binary expr);
    public TResult VisitGroupingExpr(Grouping expr);
    public TResult VisitLiteralExpr(Literal expr);
    public TResult VisitUnaryExpr(Unary expr);
  }
 public class Binary : Expr {
    public Binary(Expr Left, Token OperatorToken, Expr Right) {
        this.Left = Left;
        this.OperatorToken = OperatorToken;
        this.Right = Right;
    }
    public override TResult Accept<TResult>(IVisitor<TResult> visitor) {
      return visitor.VisitBinaryExpr(this);
    }

    public Expr Left { get; }
    public Token OperatorToken { get; }
    public Expr Right { get; }
  }
 public class Grouping : Expr {
    public Grouping(Expr Expression) {
        this.Expression = Expression;
    }
    public override TResult Accept<TResult>(IVisitor<TResult> visitor) {
      return visitor.VisitGroupingExpr(this);
    }

    public Expr Expression { get; }
  }
 public class Literal : Expr {
    public Literal(Object Value) {
        this.Value = Value;
    }
    public override TResult Accept<TResult>(IVisitor<TResult> visitor) {
      return visitor.VisitLiteralExpr(this);
    }

    public Object? Value { get; }
  }
 public class Unary : Expr {
    public Unary(Token OperatorToken, Expr Right) {
        this.OperatorToken = OperatorToken;
        this.Right = Right;
    }
    public override TResult Accept<TResult>(IVisitor<TResult> visitor) {
      return visitor.VisitUnaryExpr(this);
    }

    public Token OperatorToken { get; }
    public Expr Right { get; }
  }

  public abstract TResult Accept<TResult>(IVisitor<TResult> visitor);
}
