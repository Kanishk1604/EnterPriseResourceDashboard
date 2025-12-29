import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AssetsService } from '../../core/assets.service';
import { AuthService } from '../../core/auth.service';

@Component({
  selector: 'app-assets',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './assets.html',
  styleUrls: ['./assets.css']
})
export class AssetsComponent implements OnInit {
  page = 1;
  pageSize = 10;

  totalCount = 0;
  totalPages = 1;
  assets: any[] = [];
  error = '';

  constructor(
    private assetsService: AssetsService,
    private auth: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    if (typeof window === 'undefined') {
      return;
    }
    if (!this.auth.getToken()) {
      this.router.navigate(['/login']);
      return;
    }

    this.load();
  }

  load() {
    this.error = '';
    this.assetsService.getAssets(this.page, this.pageSize).subscribe({
      next: (res : any) => {
        console.log('ASSETS RESPONSE', res);
      console.log('ASSETS DATA', res.data);

        this.assets = res.data;
        this.totalCount = res.totalCount;
        this.totalPages = res.totalPages;
      },
      error: (err : any) => {
        console.error(err);
        this.error = 'Failed to load assets (are you logged in as Admin/Manager?)';
      }
    });
  }

  next() {
    if (this.page < this.totalPages) {
      this.page++;
      this.load();
    }
  }

  prev() {
    if (this.page > 1) {
      this.page--;
      this.load();
    }
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}