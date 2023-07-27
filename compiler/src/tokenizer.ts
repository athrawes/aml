import type { Token } from './tokens.ts'

const tokenizeCharacter =
    (type: Token['kind'], value: string) =>
        (input: string, current: number, line: number, column: number): Token | null => input[current] === value
            ? {
                kind: type,
                lineStart: line,
                lineEnd: line,
                startColumn: column,
                endColumn: column,
                value
            }
            : null;


const tokenizePattern =
    (type: Token['kind'], pattern: RegExp) =>
        (input: string, current: number, line: number, column: number): Token | null => {
            const matches = input.matchAll(pattern);
            const match = Array.from(matches).find((match) => match?.index === current);

            if (match) {
                return {
                    kind: type,
                    lineStart: line,
                    lineEnd: line,
                    startColumn: column,
                    endColumn: column + match[0].length - 1,
                    value: match[0]
                }
            }

            return null;
        }

let tokenizers = [
    tokenizeCharacter('binding', '='),
    tokenizeCharacter('groupOpen', '('),
    tokenizeCharacter('groupClose', ')'),
    tokenizeCharacter('recordOpen', '{'),
    tokenizeCharacter('recordClose', '}'),
    tokenizeCharacter('tupleOpen', '['),
    tokenizeCharacter('tupleClose', ']'),
    tokenizeCharacter('genericOpen', '<'),
    tokenizeCharacter('genericClose', '>'),
    tokenizeCharacter('valueSeparator', ','),
    tokenizeCharacter('caseArm', '|'),
    tokenizePattern('decorator', /@\w+/g),
    tokenizePattern('comment', /#[^\n]*/g),
    tokenizePattern('string', /"(?:[^"\\]|\\.)*"/g),
    tokenizePattern('infer', /(?<=\s)_(?=\s)/g),
    tokenizePattern('expressionSeparator', /\r?\n\s*\r?\n/g),
    tokenizePattern('type', /:(?=\s)/g),
    tokenizePattern('newline', /\r?\n/g),
    tokenizePattern('whitespace', /\s/g),
    tokenizePattern('function', /->/g),
    tokenizePattern('guard', /(?<!\w)\?(?!\w)/g)
]

export function* tokenize(source: string): Generator<Token> {
    let totalConsumedChars = 0;
    let line = 1;
    let column = 1;

    let buffer = '';

    while (totalConsumedChars < source.length) {
        let positionStart = totalConsumedChars;
        for (const tokenizer of tokenizers) {
            const result = tokenizer(source, totalConsumedChars, line, column);
            if (result) {
                if (buffer.length > 0) {
                    yield {
                        kind: 'identifier',
                        lineStart: line,
                        lineEnd: line,
                        startColumn: column - buffer.length,
                        endColumn: column - 1,
                        value: buffer,
                    };
                    buffer = '';
                }

                const consumedChars = result.endColumn - result.startColumn + 1;
                totalConsumedChars += consumedChars;
                yield result;
                if (result.kind === 'newline') {
                    line += 1;
                    column = 1;
                } else {
                    column += consumedChars;
                }
                break;
            }
        }
        if (positionStart === totalConsumedChars) {
            // console.log(`Could not find token for input "${source[totalConsumedChars].replace('\\', '\\\\').replace('"', '\\"')}"`, { line, column, position: totalConsumedChars });
            buffer += source[totalConsumedChars];
            totalConsumedChars += 1 // Could not find a token at this position;
            column += 1;
        }
    }
}
