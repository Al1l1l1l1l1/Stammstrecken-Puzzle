import Foundation
import StoreKit

// STP's verification bridge never finishes a transaction. Application must commit the grant first.
public typealias STPVerificationCallback = @convention(c) (UnsafeMutableRawPointer?, Int32, UnsafePointer<CChar>?, Int32) -> Void

@available(iOS 15.0, *)
private func stpVerified(_ requested: String, _ bundle: String, _ product: String, _ production: Bool) async -> (Int32, String, Int32) {
    var latest: (Date, Int32, String, Int32)?
    for await result in Transaction.all {
        guard case .verified(let transaction) = result else { continue }
        guard transaction.productID == product else { continue }
        if !requested.isEmpty && String(transaction.id) != requested { continue }
        let parts = result.jwsRepresentation.split(separator: ".", omittingEmptySubsequences: false)
        guard parts.count == 3 else { continue }
        var encoded = String(parts[1]).replacingOccurrences(of: "-", with: "+").replacingOccurrences(of: "_", with: "/")
        encoded += String(repeating: "=", count: (4 - encoded.count % 4) % 4)
        guard let data = Data(base64Encoded: encoded),
              let payload = try? JSONSerialization.jsonObject(with: data) as? [String: Any],
              payload["bundleId"] as? String == bundle,
              payload["productId"] as? String == product,
              payload["transactionId"] as? String == String(transaction.id),
              let environment = payload["environment"] as? String,
              environment == "Production" || environment == "Sandbox",
              production || environment == "Sandbox" else { continue }
        let state: Int32 = transaction.revocationDate == nil ? 1 : 2
        let sandbox: Int32 = environment == "Sandbox" ? 1 : 0
        if !requested.isEmpty { return (state, String(transaction.id), sandbox) }
        // A product-only revocation callback is insufficient: verify the newest real transaction.
        if latest == nil || transaction.purchaseDate > latest!.0 { latest = (transaction.purchaseDate, state, String(transaction.id), sandbox) }
    }
    guard let last = latest, last.1 == 2 else { return (0, "", 0) }
    return (last.1, last.2, last.3)
}

@_cdecl("stp_verify_apple_transaction")
public func stpVerifyAppleTransaction(_ transactionId: UnsafePointer<CChar>?, _ bundleId: UnsafePointer<CChar>?, _ productId: UnsafePointer<CChar>?, _ production: Int32, _ completed: STPVerificationCallback?, _ context: UnsafeMutableRawPointer?) {
    guard let callback = completed, let transactionId = transactionId, let bundleId = bundleId, let productId = productId else { return }
    // Copy pointer-backed inputs before returning across the native ABI boundary.
    let requested = String(cString: transactionId), bundle = String(cString: bundleId), product = String(cString: productId)
    let contextBits = UInt(bitPattern: context)
    if #available(iOS 15.0, *) {
        Task { @MainActor in
            let result = await stpVerified(requested, bundle, product, production != 0)
            result.1.withCString { pointer in callback(UnsafeMutableRawPointer(bitPattern: contextBits), result.0, pointer, result.2) }
        }
    } else {
        "".withCString { pointer in callback(context, 0, pointer, 0) }
    }
}
