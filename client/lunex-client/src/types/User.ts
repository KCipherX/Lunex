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
  gender: string;
  city: string;
  country: string;
  dateOfBirth: string; // YYYY-MM-DD format
  description?: string;
};

export type Member = {
  id: string;
  name: string;
  gender: string;
  description: string;
  city: string;
  country: string;
  imageUrl?: string | null;
  dateOfBirth: string;
  createdAt: string;
  lastActiveAt: string;
  photos: Photo[];
};

export type Photo = {
  id: string;
  url: string;
  publicId?: string | null;
};

export type PaginatedResult<T> = {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type MemberUpdateRequest = {
  displayName: string;
  description: string;
  city: string;
  country: string;
};

export type PhotoUploadResponse = {
  id: string;
  url: string;
  publicId?: string | null;
};