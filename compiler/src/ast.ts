import { Token } from "./tokens";

export type Expression = {
  level: number;
  expression: Array<Token | Expression>;
};

function isExpression(value: unknown): value is Expression {
  return (
    typeof value === "object" &&
    Object.getOwnPropertyNames(value).includes("level") &&
    typeof (value as any)?.level === "number" &&
    Object.getOwnPropertyNames(value).includes("expression") &&
    Array.isArray((value as any)?.expression)
  );
}

export function ast(
  tokens: Token[],
  level = 0
): [Expression[], Expression[], number, Token[]] {
  let unattachedExpressions: Expression[] = [];
  let unusedTokens: Token[] = [];
  let attachedExpressions: Expression[] = [{ level, expression: [] }];

  for (let i = 0; i < tokens.length; i++) {
    let root = attachedExpressions[attachedExpressions.length - 1];
    let token = tokens[i];
    if (token.kind === "whitespace") {
      continue;
    }

    if (token.kind === "string") {
      let [exprs, unattachedSubExpressions, subLevel, remainingTokens] = ast(
        tokens.slice(i + 1),
        level
      );
      let expr = exprs.pop();
      attachedExpressions.concat(exprs);
      level = subLevel;
      if (expr) {
        expr.expression.unshift(token);
        root?.expression.push(expr);
      }
      unattachedExpressions.push(...unattachedSubExpressions);
      tokens = remainingTokens;
      i = -1;
      continue;
    } else if (token.kind === "groupOpen") {
      let [exprs, unattachedSubExpressions, subLevel, remainingTokens] = ast(
        tokens.slice(i + 1),
        level
      );
      let expr = exprs.pop();
      attachedExpressions.concat(exprs);
      level = subLevel;
      if (expr) {
        expr.expression.unshift(token);
        root?.expression.push(expr);
      }
      unattachedExpressions.push(...unattachedSubExpressions);
      tokens = remainingTokens;
      i = -1;
      continue;
    } else if (token.kind === "genericOpen") {
      let [exprs, unattachedSubExpressions, subLevel, remainingTokens] = ast(
        tokens.slice(i + 1),
        level
      );
      let expr = exprs.pop();
      attachedExpressions.concat(exprs);
      level = subLevel;
      if (expr) {
        expr.expression.unshift(token);
        root?.expression.push(expr);
      }
      unattachedExpressions.push(...unattachedSubExpressions);
      tokens = remainingTokens;
      i = -1;
      continue;
    }

    if (token.kind === "groupClose") {
      root?.expression.push(token);
      return [
        attachedExpressions,
        unattachedExpressions,
        level,
        tokens.slice(i + 1),
      ];
    } else if (token.kind === "genericClose") {
      root?.expression.push(token);
      return [
        attachedExpressions,
        unattachedExpressions,
        level,
        tokens.slice(i + 1),
      ];
    }

    if (token.kind === "newline" && tokens[i + 1]?.kind === "newline") {
      if (!expressionContainsBinding(root)) {
        unattachedExpressions.unshift(root);
        attachedExpressions.pop();
      }

      unusedTokens.push(...tokens.slice(i + 2));
      break;
    }

    root?.expression.push(token);
  }

  return [attachedExpressions, unattachedExpressions, level, unusedTokens];
}

function expressionContainsBinding(expression: Expression): boolean {
  for (const e of expression.expression) {
    if (
      (!isExpression(e) && e.kind === "binding") ||
      (isExpression(e) && expressionContainsBinding(e))
    ) {
      return true;
    }
  }

  return false;
}
