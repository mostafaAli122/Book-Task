import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BookListComponent } from './book-list/book-list.component';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,BookListComponent],
  providers: [
    HttpClientModule
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'book-search-app';
}
