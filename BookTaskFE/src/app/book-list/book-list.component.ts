import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BookService, Book } from '../book.service';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  providers: [
    HttpClientModule
  ],
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.scss']
})
export class BookListComponent implements OnInit {
  private bookService = inject(BookService);
  books: Book[] = [];
  searchTerm: string = '';
  page: number = 1;
  pageSize: number = 10;

  ngOnInit() {
    this.getBooks();
    
  }

  getBooks() {
    this.bookService.getBooks(this.page,this.pageSize).subscribe(data => this.books = data);
  }

  searchBooks() {
    this.bookService.searchBooks(this.searchTerm).subscribe(data => this.books = data);
  }

  onSearchKey(event: KeyboardEvent) {
    if (event.key === 'Enter') {
      this.searchBooks();
    }
  }
  previousPage() {
    if (this.page > 0) {
      this.page--;
      this.getBooks();
    }
  }

  nextPage() {
    this.page++;
    this.getBooks();
  }
}
