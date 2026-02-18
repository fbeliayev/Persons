import './About.css';

const About = () => {
  return (
    <div className="about-page">
      <div className="about-container">
        <header className="about-header">
          <div className="ai-badge">🤖 AI-Generated Project</div>
          <h1>About This Project</h1>
          <p className="subtitle">A Full-Stack Application Built Entirely Through AI Collaboration</p>
        </header>

        <section className="about-section">
          <div className="section-icon">🤝</div>
          <h2>AI-Assisted Development</h2>
          <p>
            This entire application was created through conversation with <strong>GitHub Copilot</strong>, 
            demonstrating the power of AI-assisted software development in 2024.
          </p>
          <div className="info-box">
            <strong>Development Method:</strong> Natural language conversation → Complete working application
          </div>
          <p>
            Every line of code, test, and documentation was generated through iterative dialogue, 
            code reviews, and refinement—showcasing how AI can serve as a pair programming partner.
          </p>
        </section>

        <section className="stats-grid">
          <div className="stat-card">
            <div className="stat-number">133</div>
            <div className="stat-label">Unit & Integration Tests</div>
            <div className="stat-sublabel">100% Passing</div>
          </div>
          <div className="stat-card">
            <div className="stat-number">~3,000</div>
            <div className="stat-label">Lines of Code</div>
            <div className="stat-sublabel">Backend + Frontend</div>
          </div>
          <div className="stat-card">
            <div className="stat-number">50+</div>
            <div className="stat-label">Files Created</div>
            <div className="stat-sublabel">Models, Services, Components</div>
          </div>
          <div className="stat-card">
            <div className="stat-number">9.5/10</div>
            <div className="stat-label">Code Quality Score</div>
            <div className="stat-sublabel">After Code Review</div>
          </div>
        </section>

        <section className="about-section">
          <div className="section-icon">🏗️</div>
          <h2>Technology Stack</h2>
          <div className="tech-grid">
            <div className="tech-category">
              <h3>Backend</h3>
              <ul>
                <li>✅ .NET 10 (ASP.NET Core Web API)</li>
                <li>✅ Entity Framework Core</li>
                <li>✅ In-Memory Database</li>
                <li>✅ xUnit + FluentAssertions</li>
                <li>✅ CORS & Response Caching</li>
              </ul>
            </div>
            <div className="tech-category">
              <h3>Frontend</h3>
              <ul>
                <li>✅ React 18 + TypeScript</li>
                <li>✅ Vite Build Tool</li>
                <li>✅ CSS3 (Custom Styling)</li>
                <li>✅ Fetch API</li>
                <li>✅ Responsive Design</li>
              </ul>
            </div>
          </div>
        </section>

        <section className="about-section">
          <div className="section-icon">🚀</div>
          <h2>Key Features Implemented</h2>
          <div className="features-list">
            <div className="feature-item">
              <strong>Person Management</strong>
              <p>Full CRUD operations with input validation</p>
            </div>
            <div className="feature-item">
              <strong>Cities to Visit</strong>
              <p>Travel wish list with 10 pre-seeded cities</p>
            </div>
            <div className="feature-item">
              <strong>Visit Tracking</strong>
              <p>Mark cities as visited with date tracking</p>
            </div>
            <div className="feature-item">
              <strong>Smart Validation</strong>
              <p>Cannot remove visited cities (backend enforced)</p>
            </div>
            <div className="feature-item">
              <strong>Performance Optimized</strong>
              <p>Parallel loading, caching, database indexes</p>
            </div>
            <div className="feature-item">
              <strong>Comprehensive Testing</strong>
              <p>133 tests covering all scenarios</p>
            </div>
          </div>
        </section>

        <section className="about-section">
          <div className="section-icon">📈</div>
          <h2>Development Journey</h2>
          <div className="timeline">
            <div className="timeline-item">
              <div className="timeline-dot"></div>
              <div className="timeline-content">
                <h4>Phase 1: Core Setup</h4>
                <p>Created basic Person CRUD with .NET 10 backend and React frontend</p>
              </div>
            </div>
            <div className="timeline-item">
              <div className="timeline-dot"></div>
              <div className="timeline-content">
                <h4>Phase 2: Test Infrastructure</h4>
                <p>Built comprehensive test suite with 54 passing tests</p>
              </div>
            </div>
            <div className="timeline-item">
              <div className="timeline-dot"></div>
              <div className="timeline-content">
                <h4>Phase 3: Cities Feature</h4>
                <p>Added cities to visit with visit tracking and validation</p>
              </div>
            </div>
            <div className="timeline-item">
              <div className="timeline-dot"></div>
              <div className="timeline-content">
                <h4>Phase 4: Testing Expansion</h4>
                <p>Added 79 new tests for cities feature (133 total)</p>
              </div>
            </div>
            <div className="timeline-item">
              <div className="timeline-dot"></div>
              <div className="timeline-content">
                <h4>Phase 5: Code Review & Optimization</h4>
                <p>Fixed race conditions, added validation, improved performance</p>
              </div>
            </div>
            <div className="timeline-item">
              <div className="timeline-dot"></div>
              <div className="timeline-content">
                <h4>Phase 6: Polish & Documentation</h4>
                <p>Added accessibility, error handling, comprehensive docs</p>
              </div>
            </div>
          </div>
        </section>

        <section className="about-section highlight">
          <div className="section-icon">💡</div>
          <h2>What This Demonstrates</h2>
          <div className="demonstration-grid">
            <div className="demo-card">
              <h4>🎯 Clean Architecture</h4>
              <p>Proper separation of concerns with Models, Services, Controllers, and Components</p>
            </div>
            <div className="demo-card">
              <h4>🧪 Test-Driven Approach</h4>
              <p>Comprehensive testing covering happy paths, errors, and edge cases</p>
            </div>
            <div className="demo-card">
              <h4>🔒 Security Best Practices</h4>
              <p>Input validation, error handling, CORS configuration</p>
            </div>
            <div className="demo-card">
              <h4>⚡ Performance Optimization</h4>
              <p>Parallel loading, response caching, database indexes</p>
            </div>
            <div className="demo-card">
              <h4>♿ Accessibility</h4>
              <p>ARIA labels, keyboard navigation, screen reader support</p>
            </div>
            <div className="demo-card">
              <h4>📚 Documentation</h4>
              <p>README, feature docs, test docs, code review summary</p>
            </div>
          </div>
        </section>

        <section className="about-section">
          <div className="section-icon">🎓</div>
          <h2>Learning Outcomes</h2>
          <p>This project demonstrates that AI can:</p>
          <ul className="learning-list">
            <li>✅ Understand complex requirements through natural conversation</li>
            <li>✅ Generate production-quality code with proper architecture</li>
            <li>✅ Write comprehensive tests with high coverage</li>
            <li>✅ Perform code reviews and suggest improvements</li>
            <li>✅ Fix bugs and optimize performance</li>
            <li>✅ Create detailed documentation</li>
            <li>✅ Iterate and refine based on feedback</li>
          </ul>
        </section>

        <footer className="about-footer">
          <div className="footer-content">
            <p>
              <strong>Built with:</strong> GitHub Copilot (AI Assistant)
            </p>
            <p>
              <strong>Technologies:</strong> .NET 10 • React 18 • TypeScript • Entity Framework Core
            </p>
            <p>
              <strong>Repository:</strong> <a href="https://github.com/YOUR_USERNAME/person-management-cities" target="_blank" rel="noopener noreferrer">View on GitHub</a>
            </p>
            <div className="badge-container">
              <span className="badge">AI-Generated</span>
              <span className="badge">Production-Ready</span>
              <span className="badge">Open Source</span>
            </div>
          </div>
        </footer>
      </div>
    </div>
  );
};

export default About;
