/** Mirrors the server's Identity password rules so users get feedback before submitting. */
export const PASSWORD_RULES = [
  {
    kind: 'nonAlphanumeric',
    test: /[^a-zA-Z0-9]/,
    message: 'Password must have at least one non-alphanumeric character.',
  },
  { kind: 'digit', test: /[0-9]/, message: "Password must have at least one digit ('0'-'9')." },
  { kind: 'uppercase', test: /[A-Z]/, message: "Password must have at least one uppercase letter ('A'-'Z')." },
] as const;
