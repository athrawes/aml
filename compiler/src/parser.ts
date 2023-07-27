import { Token } from "./tokens.ts";
import * as R from 'ramda';
import RG from 'ramda-generators';

type Expression = {
    tokenIndex: number;
    children: Array<Token | Expression>;
};

export function parser(tokens: Array<Token>, ptr: number = 0): [number, Expression][] {
    let results: [number, Expression][] = []

    let root: Expression = { tokenIndex: ptr, children: [] };

    while (ptr < tokens.length) {
        const token = tokens[ptr];
        let nextToken: Token;
        ptr++;

        switch (token.kind) {
            case 'groupOpen':
                const result = parser(tokens, ptr);
                const [_, subexpr] = result[0];
                ptr = _;
                subexpr.children.unshift(token);
                root.children.push(...result.map(([_, e]) => e));
                break;

            case 'groupClose':
            case 'genericClose':
            case 'recordClose':
                root.children.push(token);
                results.push([ptr, root])
                return results;

            case 'newline':
                nextToken = tokens[ptr];
                if (nextToken?.kind === 'identifier') {
                    let futureToken: Token;
                    let nextPtr = ptr;
                    do {
                        futureToken = tokens[nextPtr];
                        nextPtr++;
                    } while (futureToken.kind === 'whitespace');
                    if (futureToken.kind === 'binding') {
                        root.children.push(token);
                        if (root.tokenIndex !== 0) {
                            results.push([ptr, root]);
                            return results;
                        } else {
                            results.push([ptr, root]);
                            root = { tokenIndex: 0, children: [] };
                        }
                    } else {
                        root.children.push(token)
                    }
                } else if (nextToken?.kind === 'newline') {
                    ptr++;
                    root.children.push(token);
                    root.children.push(nextToken);
                    results.push([ptr, root]);
                    root = { tokenIndex: ptr, children: [] }
                } else if (nextToken === null || typeof nextToken === 'undefined') {
                    root.children.push(token);
                    results.push([ptr, root]);
                    return results;
                }
                break;

            case 'string':
                do {
                    nextToken = tokens[ptr]
                    ptr++;
                    token.value += nextToken.value;
                    token.endColumn = nextToken.endColumn;
                    token.lineEnd = nextToken.lineEnd;
                } while (nextToken.kind !== 'string');

            default:
                root.children.push(token);
                break;
        }

    }
    results.push([ptr, root]);

    return results;
}
