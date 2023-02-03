import fs from 'node:fs';
import * as glob from 'glob';
import { tokenize } from './tokenizer';
import { promisify } from 'node:util';
import { parser } from './parser';
import RG from 'ramda-generators';

async function main() {
    const files = await promisify(glob.glob)('**/*.aml');
    for (const file of files) {
        if (!file.match(/grouped-expressions/)) continue;
        // console.log(file);
        const data = await promisify(fs.readFile)(file);
        const expression = parser(Array.from(tokenize(data.toString('utf8'))));
        console.log(JSON.stringify(expression))
    }
}
main();
