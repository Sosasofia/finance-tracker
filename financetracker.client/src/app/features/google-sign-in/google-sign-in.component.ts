import { HttpClient } from "@angular/common/http";
import { Component, NgZone, OnInit } from "@angular/core";
import { Router } from "@angular/router";

import { environment } from "../../../environments/environment";
declare const google: any;

@Component({
  selector: "app-google-sign-in",
  standalone: true,
  templateUrl: "./google-sign-in.component.html",
  styleUrls: ["./google-sign-in.component.css"],
})
export class GoogleSignInComponent implements OnInit {
  constructor(
    private ngZone: NgZone,
    private http: HttpClient,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.initializeGoogleSignIn();

    if(typeof google !== "undefined" &&  google.accounts && google.accounts.id) {
      this.initializeGoogleSignIn();
    } else {
      console.error("Google Identity Services script not loaded. Cannot initialize sign-in.");
    }
  }

  initializeGoogleSignIn() {
    google.accounts.id.initialize({
      client_id: environment.googleClientId, 
      callback: (response: any) => this.ngZone.run(() => this.handleCredentialResponse(response)),
    });

    google.accounts.id.renderButton(
      document.getElementById("google-signin-button"),
      { theme: "outline", size: "large" }, 
    );

    google.accounts.id.prompt(); // also display the One Tap dialog
  }

  handleCredentialResponse(response: any) {
    const idToken = response.credential;

    this.http
      .post(`${environment.apiUrl}/auth/google-login`, { IdToken: idToken })
      .subscribe({
        next: (res: any) => {
          localStorage.setItem("auth_token", res.token);

          this.router.navigate(["/dashboard"]);
        },
        error: (error) => {
          console.error("Login failed:", error);
        },
      });
  }
}
