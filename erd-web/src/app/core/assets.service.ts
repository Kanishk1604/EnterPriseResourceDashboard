import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AssetsService {
  private api = 'http://localhost:5083';

  constructor(private http: HttpClient) {}

  getAssets(page = 1, pageSize = 10) {
    return this.http.get<any>(
      `${this.api}/assets?page=${page}&pageSize=${pageSize}`
    );
  }
}