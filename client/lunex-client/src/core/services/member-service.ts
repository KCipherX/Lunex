import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Member, Photo, PaginatedResult, MemberUpdateRequest, PhotoUploadResponse } from '../../types/User';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  private http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/members`;

  getMembers(pageNumber = 1, pageSize = 12): Observable<PaginatedResult<Member>> {
    return this.http.get<PaginatedResult<Member>>(this.baseUrl, {
      params: { pageNumber, pageSize },
    });
  }

  getMemberById(id: string): Observable<Member> {
    return this.http.get<Member>(`${this.baseUrl}/${id}`);
  }

  getMemberPhotos(id: string): Observable<Photo[]> {
    return this.http.get<Photo[]>(`${this.baseUrl}/${id}/photos`);
  }

  updateMemberProfile(request: MemberUpdateRequest): Observable<Member> {
    return this.http.put<Member>(this.baseUrl, request);
  }

  uploadPhoto(file: File): Observable<PhotoUploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<PhotoUploadResponse>(`${this.baseUrl}/photos`, formData);
  }

  deletePhoto(photoId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/photos/${photoId}`);
  }

  setMainPhoto(photoId: string): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/photos/${photoId}/set-main`, {});
  }
}