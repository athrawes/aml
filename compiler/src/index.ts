import fs from 'node:fs';
import glob from 'glob';
import { tokenize } from './tokenizer.ts';
import { promisify } from 'node:util';
import { ast } from './ast.ts';

async function main() {
    const files = await promisify(glob)('**/*.aml');
    for (const file of files) {
        if (!file.match(/grouped-expressions/)) continue;
        // console.log(file);
        const data = await promisify(fs.readFile)(file);
        const buffer = data.toString('utf8');
        const tokens = Array.from(tokenize(buffer));
        const filteredTokens = tokens.filter(
            token => token.kind !== 'whitespace'
                  && token.kind !== 'comment'
                  && token.kind !== 'newline'
        );
        const tree = ast(tokens);
        console.log(JSON.stringify(filteredTokens))
    }
}
main();
