namespace WebBlazor.DTO;

public record TimelineSeg(
    string Lane,      // "Customer", "IVR #1", ...
    string Kind,      // "Call", "IVR", "Wait", "Wrapup" (para color/leyenda)
    int StartMin,     // 0..59
    int EndMin        // 1..60 (exclusivo)
);
