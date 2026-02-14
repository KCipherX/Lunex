export type User = {
  id: string;
  name: string;
  email: string;
  token: string;
  imageUrl?: string;
};

export type LoginCreds = {
  email: string;
  password: string;
};

export type RegisterCreds = {
  name: string;
  email: string;
  password: string;
};
