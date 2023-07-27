import { Token } from "./tokens.ts";

export enum Platform {
    POSIX,
    WASI,
    Web,
    Win32,
}

export type Program = {
    platform: Platform,
    expressions: Expression[],
}

export type Assignment = {
    name: string,
    infix: boolean,
    body: Expression[],
}

export type FunctionCall = {
    function: string,
    argument: Expression,
}

export type Type = {
    name: string,
    parameters: Type[],
}

export type Expression =
    | { id: 'assignment', value: Assignment, type: Type }
    | { id: 'function-call', value: FunctionCall, type: Type }
    | Expression[]

export function ast(tokens: Token[]): Program|null {
    let program = {
        platform: Platform.POSIX,
        expressions: [],
    }

    return program;
}

function parseExpression(tokens: Token[]): Expression|null {
    return null;
}
