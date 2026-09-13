#!/usr/bin/env node

import { createHash } from 'node:crypto';
import { readFileSync } from 'node:fs';

function assertValidString(value) {
  for (let i = 0; i < value.length; i += 1) {
    const code = value.charCodeAt(i);
    if (code >= 0xd800 && code <= 0xdbff) {
      if (i + 1 >= value.length) throw new Error('unpaired high surrogate');
      const next = value.charCodeAt(i + 1);
      if (next < 0xdc00 || next > 0xdfff) throw new Error('unpaired high surrogate');
      i += 1;
    } else if (code >= 0xdc00 && code <= 0xdfff) {
      throw new Error('unpaired low surrogate');
    }
  }
}

function parseStrictJson(source) {
  let index = 0;

  function skipWhitespace() {
    while (index < source.length && /[\u0009\u000a\u000d\u0020]/.test(source[index])) index += 1;
  }

  function parseString() {
    if (source[index] !== '"') throw new Error(`expected string at ${index}`);
    const start = index;
    index += 1;
    while (index < source.length) {
      const char = source[index];
      if (char === '"') {
        index += 1;
        const value = JSON.parse(source.slice(start, index));
        assertValidString(value);
        return value;
      }
      if (char === '\\') {
        index += 1;
        if (index >= source.length) throw new Error('unterminated escape');
        if (source[index] === 'u') {
          const code = source.slice(index + 1, index + 5);
          if (!/^[0-9a-fA-F]{4}$/.test(code)) throw new Error(`invalid unicode escape at ${index}`);
          index += 4;
        } else if (!/["\\/bfnrt]/.test(source[index])) {
          throw new Error(`invalid escape at ${index}`);
        }
      } else if (source.charCodeAt(index) <= 0x1f) {
        throw new Error(`unescaped control character at ${index}`);
      }
      index += 1;
    }
    throw new Error('unterminated string');
  }

  function parseNumber() {
    const match = source.slice(index).match(/^-?(?:0|[1-9][0-9]*)(?:\.[0-9]+)?(?:[eE][+-]?[0-9]+)?/);
    if (!match) throw new Error(`invalid number at ${index}`);
    index += match[0].length;
    if (/[.eE]/.test(match[0])) throw new Error('floating-point and exponent number tokens are forbidden');
    const value = Number(match[0]);
    if (!Number.isSafeInteger(value)) throw new Error('only interoperable safe integers are allowed');
    return value;
  }

  function parseArray() {
    const result = [];
    index += 1;
    skipWhitespace();
    if (source[index] === ']') {
      index += 1;
      return result;
    }
    while (true) {
      result.push(parseValue());
      skipWhitespace();
      if (source[index] === ']') {
        index += 1;
        return result;
      }
      if (source[index] !== ',') throw new Error(`expected comma in array at ${index}`);
      index += 1;
      skipWhitespace();
    }
  }

  function parseObject() {
    const result = Object.create(null);
    const keys = new Set();
    index += 1;
    skipWhitespace();
    if (source[index] === '}') {
      index += 1;
      return result;
    }
    while (true) {
      const key = parseString();
      if (keys.has(key)) throw new Error(`duplicate key: ${key}`);
      keys.add(key);
      skipWhitespace();
      if (source[index] !== ':') throw new Error(`expected colon at ${index}`);
      index += 1;
      skipWhitespace();
      result[key] = parseValue();
      skipWhitespace();
      if (source[index] === '}') {
        index += 1;
        return result;
      }
      if (source[index] !== ',') throw new Error(`expected comma in object at ${index}`);
      index += 1;
      skipWhitespace();
    }
  }

  function parseValue() {
    skipWhitespace();
    const char = source[index];
    if (char === '"') return parseString();
    if (char === '{') return parseObject();
    if (char === '[') return parseArray();
    if (char === '-' || /[0-9]/.test(char ?? '')) return parseNumber();
    for (const [token, value] of [['true', true], ['false', false], ['null', null]]) {
      if (source.startsWith(token, index)) {
        index += token.length;
        return value;
      }
    }
    throw new Error(`unexpected token at ${index}`);
  }

  const result = parseValue();
  skipWhitespace();
  if (index !== source.length) throw new Error(`trailing data at ${index}`);
  return result;
}

function canonicalize(value) {
  if (value === null) return 'null';
  if (value === true) return 'true';
  if (value === false) return 'false';
  if (typeof value === 'string') {
    assertValidString(value);
    return JSON.stringify(value);
  }
  if (typeof value === 'number') {
    if (!Number.isSafeInteger(value)) throw new Error('only interoperable safe integers are allowed');
    return JSON.stringify(value);
  }
  if (Array.isArray(value)) return '[' + value.map(canonicalize).join(',') + ']';
  if (typeof value === 'object') {
    const keys = Object.keys(value).sort();
    return '{' + keys.map((key) => {
      assertValidString(key);
      return JSON.stringify(key) + ':' + canonicalize(value[key]);
    }).join(',') + '}';
  }
  throw new Error('unsupported JSON value');
}

if (process.argv.length !== 3) {
  console.error('usage: node jcs_crosscheck.mjs <json-file>');
  process.exit(2);
}

try {
  const value = parseStrictJson(readFileSync(process.argv[2], 'utf8'));
  const canonical = canonicalize(value);
  const bytes = Buffer.from(canonical, 'utf8');
  const result = {
    canonicalBase64: bytes.toString('base64'),
    sha256: createHash('sha256').update(bytes).digest('hex'),
  };
  process.stdout.write(JSON.stringify(result) + '\n');
} catch (error) {
  console.error(`JCS ERROR: ${error.message}`);
  process.exit(1);
}
