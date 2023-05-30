import { Token } from "./tokens";

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

export enum Primitive {
    Boolean,
    Number,
    String,
}

export type Assignment = {
    name: string,
    infix: boolean,
    body: Expression[],
}

export type Expression =
    | Assignment
    | Primitive
    | Expression[]

export function ast(tokens: Token[]) {
}

