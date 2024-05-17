import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Book {
  bookId: number;
  bookInfo: string;
  bookInfoObj: BookInfoObj;
  lastModified: Date;
}
export interface BookInfoObj {
  bookTitle: string;
  bookDescription: string;
  author: string;
  publishDate: Date;
  coverBase64: string;
}

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private apiUrl = 'http://localhost:5264/api/Books'; 

  constructor(private http: HttpClient) {}

  getBooks(page: number, size: number): Observable<Book[]> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('size', size.toString());
    return this.http.get<Book[]>(`${this.apiUrl}/list`, { params });
  }

  searchBooks(query: string): Observable<Book[]> {
    const params = new HttpParams().set('query', query);
    return this.http.get<Book[]>(`${this.apiUrl}/search`, { params });
  }
}
