import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationService } from '../../../login/services/authentication.service';

@Component({
  selector: 'app-app-bar',
  templateUrl: './app-bar.component.html',
  styleUrls: ['./app-bar.component.scss'],
})
export class AppBarComponent implements OnInit {
  title: string;
  showLogoutButton: boolean;

  constructor(
    private readonly authenticationService: AuthenticationService,
    private readonly router: Router,
  ) {
  }

  ngOnInit(): void {
    this.authenticationService.sessionActivated$.subscribe(() => this.showLogoutButton = true);
    this.authenticationService.sessionDeactivated$.subscribe(() => this.showLogoutButton = false);
  }

  logout(): void {
    this.authenticationService.logout();
    this.router.navigate(['/']);
  }
}
